# DETI-MakerLab

<p align="center"><img src="https://firebasestorage.googleapis.com/v0/b/makerlab-b9b8c.appspot.com/o/app%2FScreenshot_1.jpg?alt=media&token=e3a065c2-4e45-49fb-abe4-77aa8808f411" width="600"></p>

DETI MakerLab app is composed by a specific database structure and a Windows C#
application and it's responsible for managing a modern and innovative room.
This room is filled with electronic components and devices, such as Arduinos, 
Raspberries, 3D printers and a network closet. The space aims at being the room
to carry on projects inside DETI@University of Aveiro.

This project was developed for HCI (Human-Computer Interaction) and
DB (Database) classes.

## Requeriments

In order to run it, first you need to use SQL Server (minimum 2012 version) to
create the database (or use an existing one since we just provide a `SCHEMA`).

Secondly, make sure you have the following toolkits added to your Visual Studio:

* [Ookii Dialogs](http://www.ookii.org/software/dialogs/)

* [Extended WPF Toolkit](http://wpftoolkit.codeplex.com/)

Finally, edit `DETI-MakerLab-DB_HCI/application/DETI-MakerLab/Helpers.cs` 
accordingly to your database settings.

--------------------------------------------------------------------------

@ Diogo Ferreira and Pedro Martins (University of Aveiro)
