import React, { Component } from "react";
import { TrackList } from "./TrackList";
import { RestDataSource } from "./RestDataSource";
import { Link } from "react-router-dom";
import { UserAlbumButton } from "./UserAlbumButton";
import { AppStateContext } from "./AppStateContext";

// Komponent der fremvises detaljer for et enkelt album
export class AlbumDisplay extends Component {

  constructor(props) {
    super(props);
    this.state = {
        album: {},
    };
    this.dataSource = new RestDataSource((err) => this.context.error.setError(true, err));
  }
  
  // Denne metode renderer komponenten
  render() {

    // DateTime-streng parses til JS-dataobjekt
    let date = new Date(this.state.album.released);

    return (              
      <div className="bg-light m-5">
        <Link to="/albums/search">                  
          <button className="btn btn-secondary m-2 float-right">
            Tilbage
          </button>
        </Link>
        <div className="container p-5">
          <div className="row">
            <div className="col-md-6 col-xs-12">
              <img src={ this.state.album.coverImageUrl || "" } alt="Album-coverbillede" />
            </div> 
            <div className="col-md-6 col-xs-12">
              <h3>{ this.state.album.title || "" }</h3>                               
              <div>{ this.state.album.artist || "" }</div>
              <div>{ date.getFullYear() || "" } </div>
              <div />
              <UserAlbumButton id={this.state.album.id} />                                      
            </div>
         </div>
        <div>
          <TrackList tracks={ this.state.album.tracks || [] } />
        </div>
      </div>
      </div>
    )     
  }

  // Data for albummet hentes, når komponenten mounter
  componentDidMount() {       
    this.dataSource.GetAlbumById(this.props.albumId, 
      data => this.setState({album: data}));
  }
}
AlbumDisplay.contextType = AppStateContext;