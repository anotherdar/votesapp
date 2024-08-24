using Microsoft.AspNetCore.Mvc;
using System.Data;
using ExcelDataReader;
using VotesApi.Models;

namespace VotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VotesController : ControllerBase
    {
        private readonly ILogger<VotesController> _logger;

        public VotesController(ILogger<VotesController> logger)
        {
            _logger = logger;
        }

        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            _logger.LogInformation("Starting file upload.");
            // confirma si hay un archivo o no.
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file was selected or the file is empty.");

                return BadRequest("Por favor, selecciona un archivo.");
            }

            // bloque try catch para evitar que la app falle
            try
            {
                // arreglo o lista local para manejar los datos de los votos
                List<Vote> votes = [];

                // leyendo el archivo .xls o .xlsx
                // en memoria
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    stream.Position = 0;

                    // para manejar diferentes tipos de caracteres ya que estamos usando un archivo
                    // .xlsx es probable que dentro de la suit de .net core no se encuentren esos
                    // caracteres especiales.
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                    // para leer el archivo .xlsx
                    // utilizando el paquete ExcelDataReader
                    using var reader = ExcelReaderFactory.CreateReader(stream);
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = false
                        }
                    });

                    var table = result.Tables[0];

                    // siclo for para recorrer la información del archivo
                    for (int i = 1; i < table.Rows.Count; i++)
                    {
                        // aqui tomamos la segunda fila ya que en la primera puede haber
                        // cabeceras tales como Candidato/votos/genero
                        var row = table.Rows[i];

                        // aquí guardamos la información en base a nuestro modelo
                        var vote = new Vote
                        {
                            Candidate = row[0]?.ToString() ?? string.Empty,
                            Votes = int.TryParse(row[1]?.ToString(), out int allVotes) ? allVotes : 0,
                            Gender = row[2]?.ToString() ?? string.Empty
                        };
                        // una vez asignada la información procedemos a guardarla en memoria
                        // que este caso seria un objeto tipo 
                        /*
                            {Votes: 10, Candidate: Manuel, Gender: hombre | mujer}
                        */
                        // una vez creada procedemos a guardarla en memoria
                        // en un forma mas simple para luego ser procesada
                        votes.Add(vote);
                    }
                }

                // Procesar los datos
                // Sum nos sumara todos los votos sean estos de mujer o hombre
                // de esta forma tendremos el total
                var totalVotes = votes.Sum(v => v.Votes);
                // aqui filtramos por genero para poder saber cuantos votos fueron
                // por hombres
                var maleVotes = votes.Where(v => v.Gender == "Hombre").Sum(v => v.Votes);
                // aqui filtramos por genero para poder saber cuantos votos fueron
                // por mujeres
                var femaleVotes = votes.Where(v => v.Gender == "Mujer").Sum(v => v.Votes);

                // votos de los candidatos
                // Aqui simplemente agrupamos por candidato y convertimos a un diccionario
                // de tal forma que podamos manejar esta información mas fácil para luego
                // sacar porcentajes
                var candidateVotes = votes
                    .GroupBy(v => v.Candidate)
                    .ToDictionary(g => g.Key, g => g.Sum(v => v.Votes));
                // mas votado
                // aquí simplemente buscamos el candidato mas votado verificando
                // que los votos del elemento anterior sea mayor o menor al siguiente en caso de ser mayo entonces
                // en la siguiente imitación del ciclo se le dice al valor presente que cambie por el elemento que sigue y 
                // asi hasta determinar quien es el candidato con mas votos
                // ej: candidato1 > candidato2 si es correcto en el siguiente paso se procede a comparar
                // candidato2 > candidato3 y asi hasta encontrar el mayor. 
                var topCandidate = candidateVotes.Aggregate((current, next) => current.Value > next.Value ? current : next).Key;
                // porcentajes
                // aui simplemente se procede a calcular el porcentaje en base a la totalidad de los votos
                // es decir votos del candidato dividido entre el total de botos multiplicado por 100
                // ejemplo: decimos que Manuel tiene 250 votos, para calcular el porcentaje sabiendo que la totalidad de votos es 700
                // (250/700) = 0.3571 * 100 = 35.71%
                var candidatePercentages = candidateVotes
                    .Select(kvp => new
                    {
                        name = kvp.Key,
                        value = Math.Round(kvp.Value / (double)totalVotes * 100, 2)
                    }).ToList();

                // una vez tenemos todos los datos procedamos a enviar la informacion
                // procesada de la siguiente manera
                /*
                {
                    "totalDeVotaciones": 615,
                    "votosDeHombres": 220,
                    "votosDeMujer": 395,
                    "mejorCandidato": "Manuela del Toro",
                    "porcentajes": [
                        {
                            "name": "Manuel Rodriguez",
                            "value": 19.51
                        },
                        {
                            "name": "Manuela del Toro",
                            "value": 48.78
                        },
                        {
                            "name": "Fracisco Perez",
                            "value": 16.26
                        },
                        {
                            "name": "Maria de Jesus",
                            "value": 15.45
                        }
                    ]
                }
                */
                return Ok(new
                {
                    totalDeVotaciones = totalVotes,
                    votosDeHombres = maleVotes,
                    votosDeMujer = femaleVotes,
                    mejorCandidato = topCandidate,
                    porcentajes = candidatePercentages
                });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error processing file.");

                // en caso de cualquier error enviamos un error al usuario diciendo que algo salio mal
                // con el status code 500 que significa que algo paso con el servidor
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error procesando el archivo: {error.Message}");
            }
        }
    }
}
