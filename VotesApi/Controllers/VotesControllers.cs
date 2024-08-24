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

                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

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
                        var row = table.Rows[i];
                        // aquí guardamos la información en base a nuestro modelo
                        var vote = new Vote
                        {
                            Candidate = row[0]?.ToString() ?? string.Empty,
                            Votes = int.TryParse(row[1]?.ToString(), out int allVotes) ? allVotes : 0,
                            Gender = row[2]?.ToString() ?? string.Empty
                        };
                        // una vez asignada la información procedemos a guardarla en memoria
                        // para luego ser procesada
                        votes.Add(vote);
                    }
                }

                // Procesar los datos
                var totalVotes = votes.Sum(v => v.Votes);
                // total de votos para hombre
                var maleVotes = votes.Where(v => v.Gender == "Hombre").Sum(v => v.Votes);
                // total de votos para mujer
                var femaleVotes = votes.Where(v => v.Gender == "Mujer").Sum(v => v.Votes);

                // votos de los candidatos
                var candidateVotes = votes
                    .GroupBy(v => v.Candidate)
                    .ToDictionary(g => g.Key, g => g.Sum(v => v.Votes));
                // mas votado
                var topCandidate = candidateVotes.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                // porcentajes
                var candidatePercentages = candidateVotes
                    .Select(kvp => new
                    {
                        name = kvp.Key,
                        value = Math.Round(kvp.Value / (double)totalVotes * 100, 2)
                    }).ToList();

                // si todo sale bien retornar un status code 200 con la información de las votaciones
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
