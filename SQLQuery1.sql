SELECT * FROM Suppliers WHERE ContactPerson IS NULL;
UPDATE Suppliers
SET ContactPerson = 'Unknown'
WHERE ContactPerson IS NULL;
