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

const Button = styled.button`
    padding: 10px;
    color: white;
    background-color: #007bff;
    border: none;
    border-radius: 4px;
    cursor: pointer;
`;

const EditItem = () => {
    const { id } = useParams();
    const navigate = useNavigate(); // użyj useNavigate
    const [item, setItem] = useState({ title: '', description: '', price: '' });

    useEffect(() => {
        // Pobierz szczegóły przedmiotu z API
        axios.get(`${API_URL}/items/${id}`)
            .then(response => {
                setItem(response.data);
            })
            .catch(error => {
                console.error('Error fetching item:', error);
            });
    }, [id]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setItem(prevState => ({
            ...prevState,
            [name]: value
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        // Zaktualizuj przedmiot
        axios.put(`${API_URL}/items/${id}`, item, {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        })
        .then(response => {
            navigate('/'); // użyj navigate zamiast history.push
        })
        .catch(error => {
            console.error('Error updating item:', error);
        });
    };

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
                type="number"
                value={item.price}
                onChange={handleChange}
            />
            <Button type="submit">Save</Button>
        </Form>
    );
};

export default EditItem;
