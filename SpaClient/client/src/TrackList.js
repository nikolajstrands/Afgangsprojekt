import React, { Component } from "react";
import { AppStateContext } from "./AppStateContext";

// Denne komponent fremviser en trackliste i en table
export class TrackList extends Component {

    render() {
        return (
            <table className="table table-dark table-hover table-sm mt-5">
                <thead>
                    <tr>
                        <th scope="col"></th>
                        <th scope="col">#</th>
                        <th scope="col">Titel</th>
                        <th scope="col">Varighed</th>
                    </tr>
                </thead>
                <tbody>
                    {
                    // tracks ordnes og fremvises
                    this.props.tracks.sort((a, b) => a.number - b.number ).map(t =>
                    <tr key={ t.number }>
                        <td>
                            <React.Fragment>
                                { this.context.user ? (
                                    <span onClick={ () => this.context.player.changeTrack(t) } >                                         
                                        <svg width="2em" height="2em" viewBox="0 0 16 16" className="bi bi-play-fill" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                                            <path d="M11.596 8.697l-6.363 3.692c-.54.313-1.233-.066-1.233-.697V4.308c0-.63.692-1.01 1.233-.696l6.363 3.692a.802.802 0 0 1 0 1.393z"/>
                                        </svg>

                                    </span>                                    
                                ) : <span /> }
                            </React.Fragment>
                        </td>
                        <td>{t.number}</td>
                        <td>{t.title}</td>
                        <td>{t.length}</td>
                    </tr>
                    )
                    }
                </tbody>
            </table>
        ) 
    }
}
TrackList.contextType = AppStateContext;