# TEST

Per effettuare i test e' necessario eseguire i seguenti comandi da powershell:
-   Per il servizio Service_DB:

    ```
    $Response = Invoke-WebRequest -Uri "http://localhost:8080/" -Method Post -ContentType 'image/jpeg' -Infile "C:\ImagePath\imageName.jpg" -UseBasicParsing
    $Response = Invoke-WebRequest 'http://localhost:8080/' -Method Get -UseBasicParsing
    $Response = Invoke-WebRequest 'http://localhost:8080/' -Method Delete -ContentType 'text/html' -Body 'imageName.jpg' -UseBasicParsing
    ```
-   Per il servizio Service_Resizer
    ```
    $Response = Invoke-WebRequest 'http://localhost:8081/' -Method Post -ContentType 'text/html' -Body 'imageName.jpg wheight heigth' -UseBasicParsing
    ```