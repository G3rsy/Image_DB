# Image_Resizer
Servizi che permettono di salvare e modificare file .jpg tramite http.

E' composto da due servizi:

- Service_DB, permette di caricare, vedere ed eliminare le foto sul server;
- Service_Resizer, permette di ridimesionare una foto preesistente sul server;

# Service_DB
Espone un Endpoint su http://localhost:8080 che si mette in attesa della connessione da parte di un client. Gestisce le richieste (POST, GET, DELETE) sequenzialmente.

## POST
Se la richiesta ricevuta tramite HTTP e' di ti POST, viene letto il body della richiesta e salvato come immagine con nome 'dateTimestamp.jpg'.

Viene inviata come risposta, il nome dell'immagine creata sul server.

## GET
In caso di richiesta GET da parte dell'utente viene direttamente restituita la lista delle eventuali immagini presenti, ignorando eventuali altri campi della richiesta.

## DELETE
Una volta ricevuta una DELETE il server controlla se e' presente un'immagine con quel nome sul server ed eventualmente viene rimossa.

# Service_Resizer
Espone un Endpoint su http://localhost:8081 che si mette in attesa delle richieste da parte del client. Si occupa di riceve un solo tipo di richieta (POST) con all'interno nome, larghezza e altezza dell'immagine. 
Ogni volta che arriva una nuova richiesta viene creato un nuovo Thread che si occupa di gestirla permetto di operare contemporaneamente su piu' documenti contemporaneamente.


# API specifications

### Service_DB
### POST
  - Metodo: POST;
  - ContentType: image/jpg;
  - body: immagine;

### GET
  - Metodo: GET;

### DELETE
  - Metodo: DELETE;
  - ContentType: text/html;
  - body: "nomeImmagine.jpg";

## Service_Receiver
### POST 
  - Metodo: POST:
  - ContentType: text/html;
  - body: "nomeImmagine.jpg larghezza(int) altezza(int)"
