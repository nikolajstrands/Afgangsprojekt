using System;
using CatalogApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Data
{
    public class CatalogRepo : ICatalogRepo
    {
        private readonly CatalogContext _db;
        private readonly DbSet<Album> _albums;
        private readonly DbSet<Track> _tracks;

        public CatalogRepo()
        {
            // Sæt database-context
            _db = new CatalogContext();
            _albums = _db.Albums;
            _tracks = _db.Tracks;
        }

        // Tilføj et album
        public int Add(Album newAlbum)
        {
            _albums.Add(newAlbum);
            return _db.SaveChanges();
        }

        // Hent alle albums
        public List<Album> GetAll()
        {
            return _albums.Include(a => a.Tracks).ToList();
        }

        // Hent albums ud fra søgestreng
        public List<Album> GetByQuery(string query)
        {
            string[] words = query.Split(' ');
           
            var albums = _albums
                .Include(a => a.Tracks)
                .Where(a => 
                    a.Artist.ToLower().Contains(words[0].ToLower()) ||
                    a.Title.ToLower().Contains(words[0].ToLower()) 
                )         
                .ToList()
                .Where(a => (
                    words.All(w =>
                        a.Artist.ToLower().Contains(w.ToLower()) ||
                        a.Title.ToLower().Contains(w.ToLower()) 
                    )
                )).ToList();

            return albums;      
             
        }

        // Hent albums ud fra en række id'er
        public List<Album> GetByIds(List<Guid> ids)
        {
            var albums = _albums.Include(a => a.Tracks).Where(a => ids.Contains(a.Id)).ToList();

            return albums;
        
        }
        
        // Hent et enkelt album ud fra id
        public Album GetById(Guid id)
        {
            var album = _albums.Include(a => a.Tracks).Select(a => a).SingleOrDefault(a => a.Id == id);               
            return album;            
        }

        // Gem et (ændret) abum
        public int Save(Album album)
        {
            _db.Entry(album).State = EntityState.Modified;
            return _db.SaveChanges();
            
        }

        // Slet et album
        public int Delete(Album album)
        {
            _db.Remove(album);
            return _db.SaveChanges();
        }

    }    
}