import React from "react";

// En statisk, stateless komponent lavet som funktions-komponent, som er appens forside
const Home = () => {

    return (
      <div className="m-5">        
        <div className="bg-light m-5">
            <div className="col text-center p-5">
              <h2>Velkommen til streamingtjenesten!</h2>
              <p>Under albums kan du søge i musikken, som tjenesten stiller til rådighed. For at se dine gemte albums og afspille musikken, skal du logge ind.</p>
            </div>
          </div>
      </div>
    );
};

export default Home;