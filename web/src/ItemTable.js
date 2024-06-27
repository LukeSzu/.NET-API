import { API_URL } from './config'; 
import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';

const getUsernameFromLocalStorage = () => {
    return localStorage.getItem('username');
};

function ItemTable() {
    const [items, setItems] = useState([]);
    const [error, setError] = useState('');
    const username = getUsernameFromLocalStorage();

    useEffect(() => {
        axios.get(`${API_URL}/items`)
            .then(response => setItems(response.data))
            .catch(error => setError('Error fetching items'));
    }, []);

    const handleDelete = async (itemId) => {
        try {
            const token = localStorage.getItem('token');
            await axios.delete(`${API_URL}/items/${itemId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            setItems(items.filter(item => item.id !== itemId));
        } catch (error) {
            console.error('Error deleting item:', error);
        }
    };

    const handleBuy = async (itemId) => {
        // Logika zakupu przedmiotu
        console.log(`Buying item with id ${itemId}`);
    };

    return (
        <div className="table-container">
            <h2 className="table-title">All items for sale</h2>
            {error && <p>{error}</p>}
            <table>
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Description</th>
                        <th>Price</th>
                        <th>Seller</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {items.map(item => (
                        <tr key={item.id}>
                            <td>{item.title}</td>
                            <td>{item.description}</td>
                            <td>{item.price}</td>
                            <td>{item.sellerUsername}</td>
                            <td>
                            {username && username === item.sellerUsername && (
                                <>
                                    <Link to={`/edit/${item.id}`}><button className="nav-item">Edit</button></Link>
                                    <button onClick={() => handleDelete(item.id) } className="nav-item">Delete</button>
                                </>
                            )}
                            {username && username !== item.sellerUsername && (
                                <button onClick={() => handleBuy(item.id)} className="nav-item">Buy</button>
                            )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            {username && (
                <>
                <div class="add-item-container">
                    <Link to="/additem" className="add-item">Add item</Link>
                </div>
                </>
            )}
            
            
        </div>
    );
}

export default ItemTable;
