import React, { Component } from "react";
import { AppStateContext } from "./AppStateContext";

// Komponent til den knap man trykker på at for at tilføje eller fjerne et album fra sit bibliotek
export class UserAlbumButton extends Component {

    render() {

        let isSaved = this.context.userAlbums.userAlbumIds.includes(this.props.id); 

        return (
            <span className="float-right" onClick={ (e) => this.context.userAlbums.toggleUserAlbum(e, this.props.id) } >
                { this.context.user ? (
                    !isSaved ? (
                        <svg width="1.5em" height="1.5em" viewBox="0 0 16 16" className="bi bi-file-plus-fill" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                        <path fill-rule="evenodd" d="M12 1H4a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V3a2 2 0 0 0-2-2zM8.5 6a.5.5 0 0 0-1 0v1.5H6a.5.5 0 0 0 0 1h1.5V10a.5.5 0 0 0 1 0V8.5H10a.5.5 0 0 0 0-1H8.5V6z"/>
                        </svg>
                    ) : (
                        <svg width="1.5em" height="1.5em" viewBox="0 0 16 16" className="bi bi-file-minus-fill" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                        <path fill-rule="evenodd" d="M12 1H4a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V3a2 2 0 0 0-2-2zM6 7.5a.5.5 0 0 0 0 1h4a.5.5 0 0 0 0-1H6z"/>
                    </svg>
                    )
                ) : ""
                }
            </span>
        )
    }    
}
UserAlbumButton.contextType = AppStateContext;