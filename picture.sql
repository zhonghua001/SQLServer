CREATE TABLE Pictures(
  Picture VARBINARY(MAX)
)

-- C#

INSERT INTO Pictures (Picture)
SELECT * 
FROM OPENROWSET(BULK N'C:\Pictures\20140101.jpg', SINGLE_BLOB) ps -- error without

/*
-- Non-Proc

//foreach (file in files)

"INSERT INTO Pictures (Picture) SELECT * FROM OPENROWSET(BULK N'" + @f + "', SINGLE_BLOB)"

*/
