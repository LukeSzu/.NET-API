import { API_URL } from './config';
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom'; // użyj useNavigate zamiast useHistory
import styled from 'styled-components';
import axios from 'axios';

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

    useEffect(() => {
        axios.get(`${API_URL}/items/${id}/details`)
            .then(response => {
                setItem(response.data);
            })
            .catch(error => {
                console.error('Error fetching item:', error);
            });
    }, [id]);


    return (
        <div>
            <h2>Item Details</h2>
            <div>
                <p><strong>Title:</strong> {item.title}</p>
                <p><strong>Description:</strong> {item.description}</p>
                <p><strong>Price:</strong> {item.price === 0 ? 'Free' : `${item.price}`}</p>
                <p><strong>Available:</strong> {item.isAvailable ? 'Yes' : 'No'}</p>
                <p><strong>City:</strong> {item.city}</p>
                <p><strong>Address:</strong> {item.address}</p>
                <p><strong>Phone Number:</strong> {item.phoneNumber}</p>
            </div>
        </div>
    );
};

export default DetailsItem;