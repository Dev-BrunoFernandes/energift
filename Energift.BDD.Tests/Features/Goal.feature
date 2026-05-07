# language: pt
Funcionalidade: Gerenciamento de Metas de Redução de Consumo
  Como usuário do Energift
  Quero criar metas de redução de consumo energético
  Para monitorar meus objetivos de eficiência e ser recompensado por alcançá-los

  Cenário: Criar meta de redução com dados válidos
    Dado que preparo uma meta com 10 porcento de redução para o usuário 1
    Quando envio uma requisição POST para "/api/goal"
    Então devo receber o status HTTP 200
    E a resposta deve conter o campo "id"

  Cenário: Criar meta com percentual de redução inválido deve retornar erro
    Dado que preparo uma meta com 0 porcento de redução para o usuário 1
    Quando envio uma requisição POST para "/api/goal"
    Então devo receber o status HTTP 500
    E a resposta deve conter o campo "error"
