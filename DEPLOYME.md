# Deploy
Per il deploy di questi servizi e' necessario:

  - Scaricare il progetto il locale;
  - Assicurarsi che il docker deamon sia attivo;
  - Eseguire la build dei due servizi con:
  ```
  docker build -t db_service --file .\DockerFiles\Dockerfile_db .
  docker build -t resizer_service  --file .\DockerFiles\Dockerfile_Res .
  ```
  - Eseguire il comando run per avviare i container:
  ```
  docker run --mount type=bind,source="$(pwd)"\Files,target=/App/Files -p 8080:8080 db_service 
  docker run --mount type=bind,source="$(pwd)"\Files,target=/App/Files -p 8081:8081 resizer_service
  ```
  Nel caso in cui la porta 8080 e 8081 non vadano bene e' necessario modificare il parametro -p x:8080, -p x:8081 sostituenso la x alla porta desiderata;
  
  Ora i servizi sono raggiungibili all'indirizzo 'http://localhost://8080' e 'http://localhost://8081' relativamente per Service_DB e per Service_Resizer
