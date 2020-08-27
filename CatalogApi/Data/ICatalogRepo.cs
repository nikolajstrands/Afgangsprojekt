using System;
using CatalogApi.Models;
using System.Collections.Generic;

namespace CatalogApi.Data
{
    // Interface for katalog-repository
    public interface ICatalogRepo
    {
        List<Album> GetAll();

        List<Album> GetByQuery(string query);

        List<Album> GetByIds(List<Guid> ids);
        int Add(Album newAlbum);
        Album GetById(Guid id);
        int Save(Album album);

        int Delete(Album album);
    }
}