import { API_URL } from './config'; 
import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const Login = () => {
    const [form, setForm] = useState({ username: '', password: '' });
    const navigate = useNavigate();

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post(`${API_URL}/auth/login`, form);
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('username', response.data.username);
            navigate('/');
        } catch (error) {
            alert('Error logging in!');
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <div>
                <label>Username:</label>
                <input type="text" name="username" value={form.username} onChange={handleChange} />
            </div>
            <div>
                <label>Password:</label>
                <input type="password" name="password" value={form.password} onChange={handleChange} />
            </div>
            <button type="submit">Login</button>
        </form>
    );
};

export default Login;
