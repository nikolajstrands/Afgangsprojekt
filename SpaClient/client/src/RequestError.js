import React, { Component } from "react";
import { AppStateContext } from './AppStateContext';


export class RequestError extends Component {

    render() {
        return (
            <div>
                <h5 className="bg-danger text-center text-white m-2 p-3">              
                    Der skete en netværksfejl: {this.context.error.message}
                    <span className="float-right close-button" onClick={ () => this.context.error.setError(false, "")}>
                        <svg width="1.5em" aria-hidden="true" focusable="false" data-prefix="far" data-icon="window-close" className="svg-inline--fa fa-window-close fa-w-16" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512">
                            <path fill="currentColor" d="M464 32H48C21.5 32 0 53.5 0 80v352c0 26.5 21.5 48 48 48h416c26.5 0 48-21.5 48-48V80c0-26.5-21.5-48-48-48zm0 394c0 3.3-2.7 6-6 6H54c-3.3 0-6-2.7-6-6V86c0-3.3 2.7-6 6-6h404c3.3 0 6 2.7 6 6v340zM356.5 194.6L295.1 256l61.4 61.4c4.6 4.6 4.6 12.1 0 16.8l-22.3 22.3c-4.6 4.6-12.1 4.6-16.8 0L256 295.1l-61.4 61.4c-4.6 4.6-12.1 4.6-16.8 0l-22.3-22.3c-4.6-4.6-4.6-12.1 0-16.8l61.4-61.4-61.4-61.4c-4.6-4.6-4.6-12.1 0-16.8l22.3-22.3c4.6-4.6 12.1-4.6 16.8 0l61.4 61.4 61.4-61.4c4.6-4.6 12.1-4.6 16.8 0l22.3 22.3c4.7 4.6 4.7 12.1 0 16.8z"></path>
                        </svg>
                    </span>
                </h5>
            </div>
         )
    }
}
RequestError.contextType = AppStateContext;