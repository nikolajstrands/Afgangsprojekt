import React from "react";

// Denne kontekst indeholder det meste state for applikationen. 
// Værdier sættes i App-komponenten
export const AppStateContext = React.createContext({
    user: null,
    userAlbums: {
        userAlbumsIds : [],
        userAlbums: [],
        getUserAlbumIds: () => {},
        getUserAlbums: () => {},
        toggleUserAlbum: () => {}
    },
    player: {
        nowPlaying: null,
        playing: false,
        togglePlay: () => {},
        changeTrack: () => {},
    },
    search: {
        query: "",
        searchResult: [],
        getSearchResult: () => {},
        setQuery: () => {}
    },
    error: {
        shown: false,
        message: "",
        setError: () => {}    
    }
})