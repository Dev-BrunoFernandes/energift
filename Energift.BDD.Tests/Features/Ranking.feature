# language: pt
Funcionalidade: Consulta de Ranking de Eficiência Energética
  Como usuário do Energift
  Quero consultar o ranking de usuários por eficiência
  Para comparar meu desempenho com outros usuários da plataforma

  Cenário: Consultar ranking mensal com usuários cadastrados
    Dado que o sistema possui usuários com consumo registrado
    Quando envio uma requisição GET para "/api/ranking?period=monthly"
    Então devo receber o status HTTP 200
    E a resposta deve ser uma lista

  Cenário: Consultar ranking anual com usuários cadastrados
    Dado que o sistema possui usuários com consumo registrado
    Quando envio uma requisição GET para "/api/ranking?period=yearly"
    Então devo receber o status HTTP 200
    E a resposta deve ser uma lista
