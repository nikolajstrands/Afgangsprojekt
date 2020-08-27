import React, { Component } from "react";
import { AlbumList } from "./AlbumList";
import { AppStateContext } from './AppStateContext';

// Komponent til at fremvise albums i brugerens bibliotek
export class MyAlbums extends Component {

    render() {

        let ids = this.context.userAlbums.userAlbumIds;

        // Vis kun albums, som stadig er gemt i brugerens bibliotek
        let shownAlbums = this.context.userAlbums.userAlbums.filter(function(album) { 
           return ids.includes(album.id);
        });

        return ( 
            <div className="m-5">
                <AlbumList albums={shownAlbums} />
            </div>                         
        ) 
    }

    // Når komponenten mountes, hentes data for de gemte albums
    componentDidMount() {
        this.context.userAlbums.getUserAlbums();
    }
    
}
MyAlbums.contextType = AppStateContext;
