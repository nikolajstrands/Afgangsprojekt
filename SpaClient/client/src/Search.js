import React, { Component } from "react";
import { AppStateContext } from './AppStateContext';

// Komponent til søgeformular
export class Search extends Component {

    constructor(props) {
        super(props);
        // Søgefeltet indhold holdes som lokal state også
        this.state = {
            query: ""
        }   
    }

    // Der kan trykkes enter i stedet for på søgeknappen
    onKeyPress = (event) => {
        if (event.charCode === 13) {
            this.submit();
          }
    }

    // Kaldes når indtastningen ændrer sig
    updateFormValue = (event) => {
        this.setState({ query: event.target.value },
            () => this.context.search.setQuery(this.state.query));
    }

    // Kaldes når der trykkes på søgeknap
    submit = () => {
        this.context.search.getSearchResult();    
    }

    render(){       
        return (
            <div className="bg-light m-5">
                <div className="col text-center p-5">                
                    <h2>Søg efter albums</h2>
                    <div className="row mt-3">
                        <div className="col-lg-12">
                            <div className="input-group ">
                                <input className="form-control "
                                    placeholder="Søg efter kunstner eller titel ..."
                                    type="text"                              
                                    name="name"
                                    value={ this.state.query }
                                    onChange={ this.updateFormValue }
                                    onKeyPress={ this.onKeyPress } />

                                <span className="input-group-btn">
                                    <button className="btn btn-primary  ml-2"
                                        onClick={ this.submit }
                                    >
                                        Søg
                                    </button> 
                                </span>    
                            </div>
                        </div>
                    </div>
                </div>
            </div>          
        )
    }

    // Der holdes os en kopi af søgestrengen i appens generelle state, som kopieres ind når formularen vises
    componentDidMount() {
        this.setState({ query: this.context.search.query});
    }
}
Search.contextType = AppStateContext;