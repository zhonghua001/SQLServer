/* 

SQL Server Inflation Example: Imports, Manipulates and Reports On Inflation Data

(This also shows the value of indexing on larger tables)

*/

CREATE TABLE StageInflation (
	SeriesID VARCHAR(20),
	Year VARCHAR(10),
	Period VARCHAR(5),
	Value DECIMAL(9,4)
)

BULK INSERT StageInflation
FROM 'C:\Users\Usr\Documents\GitHub\SQLServer\Inflation Info\testinflationdata.csv'
WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')
GO

/* In some cases, GitHub will change the format of this file, so the below code will only work:


BULK INSERT StageInflation
FROM 'C:\inflation\testinflationdata.csv'
WITH (	
	FIELDTERMINATOR = ','
	,ROWTERMINATOR = '0x0a'
)
GO

*/

CREATE TABLE Inflation(
	InflationID INT IDENTITY(1,1),
	SeriesID VARCHAR(20),
	InflationDate SMALLDATETIME,
	Value DECIMAL(9,4)
)

;WITH ShInf AS(
	SELECT s.SeriesID
		, CASE 
			WHEN Period LIKE '%M01%' THEN 01
			WHEN Period LIKE '%M02%' THEN 02
			WHEN Period LIKE '%M03%' THEN 03
			WHEN Period LIKE '%M04%' THEN 04
			WHEN Period LIKE '%M05%' THEN 05
			WHEN Period LIKE '%M06%' THEN 06
			WHEN Period LIKE '%M07%' THEN 07
			WHEN Period LIKE '%M08%' THEN 08
			WHEN Period LIKE '%M09%' THEN 09
			WHEN Period LIKE '%M10%' THEN 10
			WHEN Period LIKE '%M11%' THEN 11
			WHEN Period LIKE '%M12%' THEN 12
			ELSE 99 END AS InflationDate
		, 01
		, s.Year
		, s.Value
	FROM StageInflation s
)
INSERT INTO Inflation (SeriesID, InflationDate, Value)
SELECT si.SeriesID, si.IMonth + '-' + si.IDay + '-' + si.IYear, si.Value
FROM ShInf si

CREATE CLUSTERED INDEX [IX_IDandDate] ON [dbo].[Inflation] (
	[InflationID] ASC,
	[InflationDate] ASC
)
WITH (STATISTICS_NORECOMPUTE  = OFF
	, SORT_IN_TEMPDB = OFF
	, IGNORE_DUP_KEY = OFF
	, DROP_EXISTING = OFF
	, ONLINE = OFF
	, ALLOW_ROW_LOCKS  = ON
	, ALLOW_PAGE_LOCKS  = ON
) ON [PRIMARY]

SELECT DISTINCT SeriesID
	, COUNT(InflationDate)
FROM Inflation
GROUP BY SeriesID

DECLARE @percent TABLE (
	InflationID INT,
	SeriesID VARCHAR(20),
	InflationDate SMALLDATETIME,
	Value DECIMAL(9,4)
)

INSERT INTO @percent
SELECT *
FROM Inflation
WHERE SeriesID = 'APU0000701111'

SELECT i2.InflationDate, (((i2.Value - i1.Value)/i1.Value)*100)
FROM @percent i1, @percent i2
WHERE i1.InflationID = i2.InflationID - 1

SELECT *
FROM Inflation
WHERE SeriesID = 'APU0100703411'

-- Originally, imported the data twice.  Dropped the tables and re-ran the queries, and found no duplicate data
SELECT DISTINCT SeriesID
	, InflationDate
	, COUNT(InflationDate)
FROM Inflation
GROUP BY SeriesID, InflationDate
