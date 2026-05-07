# language: pt
Funcionalidade: Gerenciamento de Consumo de Energia
  Como usuário do Energift
  Quero registrar e consultar meu consumo de energia
  Para acompanhar minha eficiência energética e acumular WattCoins

  Cenário: Registrar consumo de energia com dados válidos
    Dado que preparo um registro de consumo com 200 kWh para o usuário 1
    Quando envio uma requisição POST para "/api/consumo"
    Então devo receber o status HTTP 200
    E a resposta deve conter o campo "id"

  Cenário: Consultar histórico de consumo paginado por usuário
    Dado que o serviço retorna uma página de consumos para o usuário 1
    Quando envio uma requisição GET para "/api/consumo?usuarioId=1"
    Então devo receber o status HTTP 200
    E a resposta deve conter o campo "items"

  Cenário: Calcular WattCoins ao reduzir consumo energético
    Dado que preparo um cálculo de WattCoins com 180 kWh para o usuário 1
    Quando envio uma requisição POST para "/api/consumo/calculate-coins"
    Então devo receber o status HTTP 200
    E a resposta deve conter o campo "awarded"
