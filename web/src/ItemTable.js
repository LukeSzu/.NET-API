import { API_URL } from './config'; 
import React, { useEffect, useState, useContext } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';
import { useExchangeRates } from './ExchangeRatesProvider';
import { CurrencyContext } from './App'; 

const getUsernameFromLocalStorage = () => {
    return localStorage.getItem('username');
};

function ItemTable() {
    const [items, setItems] = useState([]);
    const [error, setError] = useState('');
    const username = getUsernameFromLocalStorage();
    const exchangeRates = useExchangeRates();
    const [selectedCurrency] = useContext(CurrencyContext); 

    const convertPrice = (priceInPLN) => {
        const rate = exchangeRates[selectedCurrency];
        return (priceInPLN / rate).toFixed(2);
      };

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
                        <th>Add time</th>
                        <th>Seller</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {items.map(item => (
                        <tr key={item.id}>
                            <td>{item.title}</td>
                            <td>{item.description}</td>
                            <td>{item.price === 0 ? 'Free' : `${convertPrice(item.price)} ${selectedCurrency}`}</td>
                            <td>{formatDateTime(item.addTime)}</td> 
                            <td>{item.sellerUsername}</td>
                            <td>
                            <Link to={`/details/${item.id}`}><button className="nav-item">Details</button></Link>
                            {username && username === item.sellerUsername && (
                                <>
                                    <Link to={`/edit/${item.id}`}><button className="nav-item">Edit</button></Link>
                                    <button onClick={() => handleDelete(item.id) } className="nav-item">Delete</button>
                                </>
                            )}
                            
                            
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            {username && (
                <>
                <div className="add-item-container">
                    <Link to="/additem" className="add-item">Add item</Link>
                </div>
                </>
            )}
        </div>
    );
}

export default ItemTable;
