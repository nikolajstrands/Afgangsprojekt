import React from 'react';
import { AuthenticationContext } from '@axa-fr/react-oidc-context';
import { Link, NavLink } from "react-router-dom";

// En header-komponent som er på alle views
export default () => (
  <header>
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
      <AuthenticationContext.Consumer>
        { props =>
        <React.Fragment>
          <img className="logo" alt="Music logo" src={ process.env.PUBLIC_URL + "/music.png" } />  
          <Link className="navbar-brand" to="/albums">STREAMINGTJENESTE</Link>
          <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse navbar-collapse" id="navbarSupportedContent">
            <ul className="navbar-nav mr-auto">
              <li className="nav-item">
                <NavLink className="nav-link" 
                          activeClassName="active" 
                          to="/albums/search">Albums <span className="sr-only">(current)</span></NavLink>
              </li>  
                {/* Afhængig af om brugeren er logget ind eller ej, vises der ekstra links */}                                   
                { props.oidcUser ? (
                  <React.Fragment>
                    <li className="nav-item">
                      <NavLink className="nav-link"
                                activeClassName="active"  
                                to="/myalbums">Mine albums <span className="sr-only">(current)</span></NavLink> 
                    </li> 
                  </React.Fragment>                                        
                ) : ( "")}          
            </ul>
            { props.oidcUser ? (
                  <React.Fragment>
                    <div className="text-white pl-3 pr-3">{props.oidcUser.profile.name}</div>
                    <button className="btn btn-danger my-2 my-lg-0" onClick={props.logout}>Log ud</button>
                  </React.Fragment>                
                ) : (
                    <button className="btn btn-primary my-2 my-lg-0" onClick={props.login}>Log ind</button>
                )
            }
          </div>
        </React.Fragment> 
        }
      </AuthenticationContext.Consumer>
    </nav>
  </header>
);