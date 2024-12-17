#!/bin/bash

echo "Waiting for SQL Server to start..."
sleep 15s

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -d master -i /init-scripts/schema.sql

echo "Schema initialized!"
