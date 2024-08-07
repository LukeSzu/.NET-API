import { API_URL } from './config'; 
import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode'; // Poprawiony import
import useLoginTimeout from './loginTimeout';

const Login = () => {
    const [form, setForm] = useState({ username: '', password: '' });
    const navigate = useNavigate();
    const [message, setMessage] = useState('');
    const [loginSuccess, setLoginSuccess] = useState(false);
    const { setLoginTimeout, clearLoginTimeout } = useLoginTimeout();

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post(`${API_URL}/auth/login`, form);
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('username', response.data.username);
            const token = localStorage.getItem('token');

            //decoding token for expiration date
            const { exp } = jwtDecode(token);
            const expirationTime = exp * 1000;

            const currentTime = new Date().getTime();
            const timeUntilExpiration = expirationTime - currentTime;

            clearLoginTimeout();
            //auto logout after expiration
            setLoginTimeout(() => {
                localStorage.removeItem('token');
                localStorage.removeItem('username');
                navigate("/login");
            }, timeUntilExpiration);

            setMessage("Login success!");
            setLoginSuccess(true);
        } catch (error) {
            setMessage("Bad login or password");
        }
    };
    //Navigate after 3 seconds if login was successful
    useEffect(() => {
        if (loginSuccess) {
            const timeoutId = setTimeout(() => {
                navigate('/');
            }, 3000); 
            
            return () => clearTimeout(timeoutId); 
        }
    }, [loginSuccess, navigate]);

    return (
        <form onSubmit={handleSubmit} class="form-container">
            <div >
                <label><span class="required">*</span>Username:</label>
                <input type="text" name="username" value={form.username} onChange={handleChange} required />
            </div>
            <div>
                <label><span class="required">*</span>Password:</label>
                <input type="password" name="password" value={form.password} onChange={handleChange} required />
            </div>
            <button type="submit">Login</button>
            {message && <p class="message">{message}</p>}
        </form>
    );
};

export default Login;
