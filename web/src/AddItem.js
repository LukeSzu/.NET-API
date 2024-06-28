import { API_URL } from './config';
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom'; // użyj useNavigate zamiast useHistory
import styled from 'styled-components';
import axios from 'axios';

const Form = styled.form`
    display: flex;
    flex-direction: column;
    max-width: 400px;
    margin: auto;
`;

const Label = styled.label`
    margin-bottom: 8px;
    font-weight: bold;
`;

const Input = styled.input`
    margin-bottom: 16px;
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
`;

const Select = styled.select`
    margin-bottom: 16px;
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
`;

const Button = styled.button`
    padding: 10px;
    color: white;
    background-color: #007bff;
    border: none;
    border-radius: 4px;
    cursor: pointer;
`;

const AddItem = () => {
    const { id } = useParams();
    const navigate = useNavigate(); 
    const [item, setItem] = useState({ title: '', description: '', price: '', isAvailable: false });
    const [message, setMessage] = useState('');
    const [actionSuccess, setActionSuccess] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;

        setItem(prevItem => ({
            ...prevItem,
            [name]: value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if(item.price == ''){
            item.price = '0'
        }
        const editData = {
            title: item.title,
            description: item.description,
            price: item.price,
        }

        try{
            const response = await axios.post(`${API_URL}/items`, editData, {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });

            setMessage('Item added successfully!');
            setActionSuccess(true);
        }catch(error){
            if (error.response && error.response.data && Array.isArray(error.response.data)) {
                const firstErrorDescription = error.response.data[0].description;
                setMessage(firstErrorDescription);
            } else {
                setMessage("Another error ¯\\_(ツ)_/¯");
            }
        }
    };

    useEffect(() => {
        if (actionSuccess) {
            const timeoutId = setTimeout(() => {
                navigate('/');
            }, 3000); 
            
            return () => clearTimeout(timeoutId); 
        }
    }, [actionSuccess, navigate]);

    return (
        <Form onSubmit={handleSubmit}>
            <Label htmlFor="title">Title</Label>
            <Input
                id="title"
                name="title"
                type="text"
                value={item.title}
                onChange={handleChange}
            />
            <Label htmlFor="description">Description</Label>
            <Input
                id="description"
                name="description"
                type="text"
                value={item.description}
                onChange={handleChange}
            />
            <Label htmlFor="price">Price</Label>
            <Input
                id="price"
                name="price"
                type="decimal"
                value={item.price}
                onChange={handleChange}
            />
            <Button type="submit">Add</Button>
            {message && <p class="message">{message}</p>}
        </Form>
    );
};

export default AddItem;
