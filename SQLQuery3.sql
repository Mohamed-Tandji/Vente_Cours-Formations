

CREATE TABLE [dbo].[loginProf] (
    [us]        VARCHAR (50) NOT NULL,
    [pw]        VARCHAR (50) NOT NULL,
    [id_enseignants] INT          NOT NULL,
    [statut]    VARCHAR (50) NOT NULL,
    CONSTRAINT [FK_login_enseignants] FOREIGN KEY ([id_enseignants]) REFERENCES [dbo].enseignants ([Id])
);