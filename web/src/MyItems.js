import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { API_URL } from './config';

const MyItems = () => {
    const [items, setItems] = useState([]);
    const username = localStorage.getItem('username');

    useEffect(() => {
        const fetchItems = async () => {
            try {
                const response = await axios.get(`${API_URL}/items`);
                const userItems = response.data.filter(item => item.sellerUsername === username);
                setItems(userItems);
            } catch (error) {
                console.error('Error fetching items:', error);
            }
        };

        fetchItems();
    }, [username]);

    return (
        <div className="my-items-container">
            <h2>My Items</h2>
            {items.length > 0 ? (
                <ul>
                    {items.map(item => (
                        <li key={item.id}>{item.title} - {item.description} - ${item.price}</li>
                    ))}
                </ul>
            ) : (
                <p>You have no items listed.</p>
            )}
        </div>
    );
};

export default MyItems;