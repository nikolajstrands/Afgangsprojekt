import React, { Component } from "react";
import { Link } from "react-router-dom";
import { UserAlbumButton } from "./UserAlbumButton";
import { AppStateContext } from "./AppStateContext";

// Komponent der fremviser en liste af albums
export class AlbumList extends Component {

  render(){
      
  if (this.props.albums.length === 0)
  {
    return (
      <div />
    )
  }
  return (
    <div className="bg-light m-5">
      <div className="container pt-3 pb-3 mb-5">
        <div className="row">
        { this.props.albums.map(album => (
          <div key={ album.id } className="col-12 col-sm-6 col-md-4 col-lg-3 album">             
            <Link to={ "/albums/" + album.id }>
              <div className="bg-dark m-2">
                <div className="p-1"> 
                  <img className=" w-100" src={ album.coverImageUrl } alt="Album-coverbillede" />
                </div>
                <div className="text-white p-2 pb-3">
                  <div>
                    { album.artist }
                    <UserAlbumButton 
                      id={album.id}
                    />                             
                  </div>
                  <div className="album-title">{ album.title }</div>                                       
                </div>
              </div>
            </Link>
          </div>      
          )
        )}
        </div>   
      </div>
    </div>           
  )
  }
}
AlbumList.contextType = AppStateContext;
