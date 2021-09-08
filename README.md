# Image_Resizer
Servizi che permettono di salvare e modificare file .jpg tramite http.

E' composto da due servizi:

- Service_DB, permette di caricare, vedere ed eliminare le foto sul server;
- Service_Resizer, permette di ridimesionare una foto preesistente sul server;

Per la gestione delle richieste http ho utilizzato HttpListener su entrami i servizi.
Per la gestione delle immagini ho utilizzato la libreria System.Drawing che implementa la classe Bitmap che e' stata utilizzata per manipolare le immagine.

# Service_DB
Espone un Endpoint su http://localhost:8080 che si mette in attesa della connessione da parte di un client. Gestisce le richieste (POST, GET, DELETE) sequenzialmente.

## POST
Se la richiesta ricevuta tramite HTTP e' di tipo POST, viene letto il body della richiesta e salvato come immagine con nome 'dateTimestamp.jpg'.

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

Come Risposte a queste richieste, vale per entrambe i servizi, vengono inviati dei messaggi di testo che cominicano la riuscita dell'operazione. Nel caso della GET, viene restituita un testo con nome, larghezza e altezza delle eventuali foto presenti.

#Dockerizzazione

Per effettuare la dockerizzazione ho creto un dockerfile per servizio che partendo dall'immagine asp.net di Microsft mi permettono di crere il giusto ambiente per rendere funzionante il server su container. Per la condivisione dei dati ho riservato un volume che viene assegnato ai container in fase di run, che permette di mantere le immagini memorizzate anche in caso di malfunzionamente da parte delle stesse. 

Nel DockerFile vengono installate anche alcune dipendenze che mancavano per la corretta esecuzione delle funzione nel namespace System.Drawing per la gestione delle immagini.

