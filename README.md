
# Nachrichtenaustausch (Messaging) — Implementierung

Mit diesem Projekt wird Beispielsweise die Kommunikation von Clients über ActiveMQ Artemis als Message Broker dargestellt.

## Docker Container starten

Zuerst sollte der ActiveMQ Artemis container mit folgendem Befehl gestartet werden

```powershell
docker run --detach --name mycontainer -p 61616:61616 -p 8161:8161 -p 5672:5672 --rm apache/activemq-artemis:latest
```

## Clients starten

Nun können beliebig viele UserClients und BörsenClients über die .exe-dateien gestartet werden.


AktienClient/AktienClient/bin/Debug/net8.0-windows/UserClient.exe
BörsenServer/BörsenServer/bin/Debug/net8.0/BörsenServer.exe

## UserClient

Oben links können die Aktien ausgewählt werden, welche überwacht werden sollen. Um diese anzuzeigen muss der Button "Update selection" gedrückt werden.

Um nun eine Buy oder Sell Order abzusetzen, muss rechts die jeweilige Börse ausgewählt werden. Danach kann über Buy bzw Sell der Order Request an die ausgewählte Börse zu setzen.

Daraufhin erscheint die Order im Textfeld unten links. Der Status ist anfangs auf "pending" gesetzt. Wenn nun die Response des jeweilgen BörsenServers eingeht, d.h. die Order bearbeitet wurde, wird der Status der zugehörigen Bestellung automatisch auf "successful" gesetzt.

## BörsenServer

Nachdem starten muss der Name der Börse eingegeben werden.
Da nur 3 verschiedene Börsen auf dem UserClient unterstützt werden, sollte der des BörsenServers einer der folgenden sein: "Frankfurt", "Stuttgart" oder "München".

Der BörsenServer sendet nun alle 30 Sekunden die aktuellen Aktienpreise der 5 Beispielaktien an das "stockprice"-topic in ActiveMQ Artemis.
Die Preise entstammen der API von finnhub.io.

Desweiteren lauscht der BörsenServer nach Order Requests auf der jeweiligen Message Queue mit dem Namen "orders.{BörsenServerName}".\
Wenn ein Order Request eingeht, sendet der BörsenServer nach 15 Sekunden ein Response mit der Order ID an den jeweiligen Client über eine Queue um die Order zu bestätigen.
