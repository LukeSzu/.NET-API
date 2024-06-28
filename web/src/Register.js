import { API_URL } from './config'; 
import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const Register = () => {
    const [form, setForm] = useState({ username: '', password: '', Repassword: '', city: '', email: '', phonenumber: '', address: '' });

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };
    const navigate = useNavigate();
    const [message, setMessage] = useState('');
    const [registrationSuccess, setRegistrationSuccess] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (form.password !== form.Repassword) {
            setMessage('Passwords do not match!');
            return; 
        }
        const { Repassword, ...dataToSend } = form;
        try {
            const response = await axios.post(`${API_URL}/auth/register`, JSON.stringify(dataToSend), {
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            setMessage('User registered successfully!');
            setRegistrationSuccess(true);
        } catch (error) {
            if (error.response && error.response.data && Array.isArray(error.response.data)) {
                const firstErrorDescription = error.response.data[0].description;
                setMessage(firstErrorDescription);
            } else {
                setMessage("Another error ¯\_(ツ)_/¯");
            }
        }
    };

    useEffect(() => {
        if (registrationSuccess) {
            const timeoutId = setTimeout(() => {
                navigate('/login');
            }, 3000); 
            
            return () => clearTimeout(timeoutId); 
        }
    }, [registrationSuccess, navigate]);

    return (
        <form onSubmit={handleSubmit} class="form-container">
            <div>
                <label>Username:</label>
                <input type="text" name="username" value={form.username} onChange={handleChange} />
            </div>
            <div>
                <label>Password:</label>
                <input type="password" name="password" value={form.password} onChange={handleChange} />
            </div>
            <div>
                <label>Retype password:</label>
                <input type="password" name="Repassword" value={form.Repassword} onChange={handleChange} />
            </div>
            <div>
                <label>E-mail</label>
                <input type="text" name="email" value={form.email} onChange={handleChange} />
            </div>
            <div>
                <label>Phone number</label>
                <input type="text" name="phonenumber" value={form.phonenumber} onChange={handleChange} pattern='[0-9]{9}' />
            </div>
            <div>
                <label>City:</label>
                <input type="text" name="city" value={form.city} onChange={handleChange} />
            </div>
            <div>
                <label>Address:</label>
                <input type="text" name="address" value={form.address} onChange={handleChange} />
            </div>
            <button type="submit">Register</button>
            {message && <p class="message">{message}</p>}
        </form>
    );
};

export default Register;
