interface VotesInfo {
    totalDeVotaciones: number;
    votosDeHombres:    number;
    votosDeMujer:      number;
    mejorCandidato:    string;
    porcentajes:       Porcentaje[];
}

interface Porcentaje {
    name:  string;
    value: number;
}
