using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp
{

    public class DataReader
    {
        private IEnumerable<ImportedObject> _importedObjects;

        public void ImportData(string fileToImport)
        {
            _importedObjects = new List<ImportedObject> { new ImportedObject() };

            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            for (var i = 0; i <= importedLines.Count; i++)
            {
                var importedLine = importedLines[i];
                var values = importedLine.Split(';');
                var importedObject = new ImportedObject
                {
                    Type = values[0],
                    Name = values[1],
                    Schema = values[2],
                    ParentName = values[3],
                    ParentType = values[4],
                    DataType = values[5],
                    IsNullable = values[6]
                };
                ((List<ImportedObject>)_importedObjects).Add(importedObject);
            }
        }

        public void PrintData()
        {
            foreach (var database in _importedObjects)
            {
                if (database.Type != "DATABASE") continue;
                Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                // print all database's tables
                foreach (var table in _importedObjects)
                {
                    if (table.ParentType.ToUpper() != database.Type) continue;
                    if (table.ParentName != database.Name) continue;
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                    // print all table's columns
                    foreach (var column in _importedObjects)
                    {
                        if (column.ParentType.ToUpper() != table.Type) continue;
                        if (column.ParentName == table.Name)
                        {
                            Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                        }
                    }
                }
            }
        }

        public void ClearData()
        {
            foreach (var importedObject in _importedObjects)
            {
                importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.Schema = importedObject.Schema.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.ParentName = importedObject.ParentName.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.ParentType = importedObject.ParentType.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
            }
        }

        public void AssignChildren()
        {
            for (var i = 0; i < _importedObjects.Count(); i++)
            {
                var importedObject = _importedObjects.ToArray()[i];
                foreach (var impObj in _importedObjects)
                {
                    if (impObj.ParentType != importedObject.Type) continue;
                    if (impObj.ParentName == importedObject.Name)
                    {
                        importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
                    }
                }
            }
        }
    }

    internal class ImportedObject : ImportedObjectBaseClass
    {
        public string Name
        {
            get;
            set;
        }
        public string Schema;

        public string ParentName;
        public string ParentType
        {
            get; set;
        }

        public string DataType { get; set; }
        public string IsNullable { get; set; }

        public double NumberOfChildren;
    }

    internal class ImportedObjectBaseClass
    {
        public string Type { get; set; }
    }
}
