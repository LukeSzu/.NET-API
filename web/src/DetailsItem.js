import { API_URL } from './config';
import React, { useState, useEffect, useContext  } from 'react';
import { useParams, useNavigate } from 'react-router-dom'; // użyj useNavigate zamiast useHistory
import styled from 'styled-components';
import axios from 'axios';
import { useExchangeRates } from './ExchangeRatesProvider';
import { CurrencyContext } from './App'; // Importuj kontekst waluty
import { Link } from 'react-router-dom';

const DetailsItem = () => {
    const { id } = useParams();
    const [item, setItem] = useState({ 
        title: '', 
        description: '', 
        price: '', 
        isAvailable: false, 
        City: '', 
        Address: '', 
        PhoneNumber: ''  
    });

    const exchangeRates = useExchangeRates();
    const [selectedCurrency] = useContext(CurrencyContext); // Pobierz walutę z kontekstu

    const convertPrice = (priceInPLN) => {
        const rate = exchangeRates[selectedCurrency];
        return (priceInPLN / rate).toFixed(2);
      };

    useEffect(() => {
        axios.get(`${API_URL}/items/${id}/details`)
            .then(response => {
                setItem(response.data);
            })
            .catch(error => {
                console.error('Error fetching item:', error);
            });
    }, [id]);

    const formatDateTime = (dateTimeString) => {
        const date = new Date(dateTimeString);
      
        const day = date.getDate().toString().padStart(2, '0');
        const month = (date.getMonth() + 1).toString().padStart(2, '0'); 
        const year = date.getFullYear(); 
      
        const hours = date.getHours().toString().padStart(2, '0'); 
        const minutes = date.getMinutes().toString().padStart(2, '0'); 
        const formattedDateTime = `${day}-${month}-${year} ${hours}:${minutes}`;
      
        return formattedDateTime;
      };
      const formattedAddress = (address) => {
        if (!address) return 'bad'; // Jeśli address jest puste, zwraca 'bad'
      
        const indexOfFirstSlash = address.indexOf('/'); // Znajduje indeks pierwszego ukośnika
      
        if (indexOfFirstSlash === -1) {
          return address; // Jeśli brak ukośnika, zwraca oryginalny address
        } else {
          return address.substring(0, indexOfFirstSlash); // Zwraca fragment adresu przed pierwszym ukośnikiem
        }
      };

    return (
        <div className="item-details-container">
      <h2>Item Details</h2>
      <div>
        <p><strong>Title:</strong> {item.title}</p>
        <p><strong>Description:</strong> {item.description}</p>
        <p><strong>Price:</strong> {item.price === 0 ? 'Free' : `${convertPrice(item.price)} ${selectedCurrency}`}</p>
        <p><strong>Available:</strong> {item.isAvailable ? 'Yes' : 'No'}</p>
        <p><strong>City:</strong> {item.city}</p>
        <p><strong>Address:</strong> {item.address}</p>
        <p><strong>Phone Number:</strong> {item.phoneNumber}</p>
        <p><strong>Add time:</strong> {formatDateTime(item.addTime)}</p>
        <p><strong>Map:</strong><Link to={`/map/${item.city}/${formattedAddress(item.address)}`}><button className="nav-item">Map</button></Link></p>
      </div>
    </div>
    );
};

export default DetailsItem;