# fs
An EF Core &amp; MariaDB file storage. Latest development in dev branch. Currently connection string can be provided in app.config file in project root, example contents:

<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <appSettings>
    <add key="ConnString" value="..." />
  </appSettings>
</configuration>
