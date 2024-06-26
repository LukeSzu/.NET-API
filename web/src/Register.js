import { API_URL } from './config'; 
import React, { useState } from 'react';
import axios from 'axios';

const Register = () => {
    const [form, setForm] = useState({ username: '', password: '' });

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };
    

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await axios.post(`${API_URL}/auth/register`, form);
            alert('User registered successfully!');
        } catch (error) {
            alert('Error registering user!');
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
            <button type="submit">Register</button>
        </form>
    );
};

export default Register;
