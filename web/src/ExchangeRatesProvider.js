import React, { createContext, useContext, useState, useEffect } from 'react';
import axios from 'axios';

const ExchangeRatesContext = createContext();

export const useExchangeRates = () => useContext(ExchangeRatesContext);

const ExchangeRatesProvider = ({ children }) => {
  const [exchangeRates, setExchangeRates] = useState({
    PLN: 1,
    EUR: null,
    USD: null,
    GBP: null
  });

  useEffect(() => {
    const fetchExchangeRates = async () => {
      try {
        const responseEuro = await axios.get('http://api.nbp.pl/api/exchangerates/rates/A/EUR/');
        const responseUSD = await axios.get('http://api.nbp.pl/api/exchangerates/rates/A/USD/');
        const responseGBP = await axios.get('http://api.nbp.pl/api/exchangerates/rates/A/GBP/');

        setExchangeRates({
          PLN: 1,
          EUR: responseEuro.data.rates[0].mid,
          USD: responseUSD.data.rates[0].mid,
          GBP: responseGBP.data.rates[0].mid
        });
      } catch (error) {
        console.error('Error fetching exchange rates:', error);
      }
    };

    fetchExchangeRates();
  }, []);

  return (
    <ExchangeRatesContext.Provider value={exchangeRates}>
      {children}
    </ExchangeRatesContext.Provider>
  );
};

export default ExchangeRatesProvider;
