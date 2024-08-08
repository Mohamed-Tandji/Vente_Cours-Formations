DECLARE @Username NVARCHAR(100) = 'MahamadouTandjigora';
DECLARE @Password NVARCHAR(100) = 'Mahamadou89';
DECLARE @Statut NVARCHAR(50) = 'True';
DECLARE @id_eleves INT = 1;

UPDATE login
SET us = @Username, pw = @Password, statut = @Statut
WHERE id_eleves = @id_eleves;
