import React, { Component } from "react";
import { AlbumList } from "./AlbumList";
import { Search } from "./Search";
import { AppStateContext } from "./AppStateContext";

// En komponent for album-søge-viewet
export class AlbumSearch extends Component {

    render() {
            return (
                <div className="m-5">
                    <Search />
                    <AlbumList albums={this.context.search.searchResult} />
                </div>
            )  
    }
}
AlbumSearch.contextType = AppStateContext;
