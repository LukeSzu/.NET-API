import { API_URL } from './config'; 
import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import useLoginTimeout from './loginTimeout';

const getUsernameFromLocalStorage = () => {
    return localStorage.getItem('username');
};

function MyItemTable() {
    const [items, setItems] = useState([]);
    const [error, setError] = useState('');
    const username = getUsernameFromLocalStorage();
    const { setLoginTimeout, clearLoginTimeout } = useLoginTimeout();
    const navigate = useNavigate();

    useEffect(() => {
        axios.get(`${API_URL}/items/myitems`, {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        })
            .then(response => setItems(response.data))
            .catch(error => {
                localStorage.removeItem('username');
                localStorage.removeItem('token');
                clearLoginTimeout();
                navigate('/login');
                setError('Error fetching items')
            });
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


    return (
        <div className="table-container">
            <h2 className="table-title">Items for Sale</h2>
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
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default MyItemTable;
