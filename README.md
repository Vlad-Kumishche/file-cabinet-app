# File Cabinet App
App for storing, managing and generating file cabinet records. It's like a database, with only one table.
<br />
<br />
Command line options:
- `--storage`, `-s` - Type of storage (In-memory or file)
- `--validation-rules`, `-v` - Default validation rules or custom
- `--use`, `-u` - Enables services like Logger or Meter

Implemented storage options:
- XML file
- CSV file
- In-memory

Implemented commands:<br /><br />
Select: 
```
select id, firstname, LastName where firstname = 'John' and lastname = 'Doe'
select * where FavoriteLetter = 'g' or height = '168'
select *
```
 Insert:
```
insert (firsName, lastName, dateOfBirth, height, cashSavings, favoriteLetter) values (Vlad, Who, 08/22/2002, 186, 300, F) 
```
Update:
```
update set firstname = 'John', lastname = 'Doe' where height = '182' and dateofbirth = '5/18/1986'
update set FavoriteLetter = 'A' where FirstName = 'Tomas' or favoriteetter = 'f'
update set CashSavings = '100' where *
```
Delete:
```
delete where firstName = 'Denis'
delete where firstName = 'Denis' and Height = '164'
delete *
```
Export:
```
export csv rec.csv
export xml records.xml
```
Import:
```
import csv rec.csv
import xml records.xml
```
Create - Creates a new record <br />
Stat - Prints statistics on records <br />
Purge - Defragments the data file <br />
Help - Prints the help screen <br />
Exit - Exits the application <br />
<br />
Applied design patterns:
- Builder - ValidatorBuilder
- Decorator - ServiceLogger, ServiceMeter
- Chain of Responsibilities - CommandHandlers
- Strategy - Services, Readers, Writers, Printers
- Memento - FileCabinetServiceSnapshot

## _File Cabinet Generator_
Generates records and saves them to the file.
Command line options:
- `--output-type`, `-t` - Type of file
- `--output`, `-o` - Name of output file
- `--records-amount`, `-a` - Amount of generated records
- `--start-id`, `-i` - Id from which to start numbering records
