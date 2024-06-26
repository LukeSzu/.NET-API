import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link, useNavigate } from 'react-router-dom';
import ItemTable from './ItemTable';
import EditItem from './EditItem';
import Login from './Login';
import Register from './Register';
import MyItems from './MyItems';
import './App.css';

function Navigation() {
  const username = localStorage.getItem('username');
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('username');
    localStorage.removeItem('token');
    navigate('/login');
  };

  return (
    <nav>
      <ul>
        <li>
          <Link to="/" className="nav-item">Home</Link>
        </li>
        
        {username ? (
            <>
            <li>
                <Link to="/myitems" className="nav-item">My items</Link>
            </li>
            <li>
                <button onClick={handleLogout} className="nav-item">Logout</button>
            </li>
          </>
        ): (
            <>
            <li>
            <   Link to="/login" className="nav-item">Login</Link>
            </li>
            <li>
                <Link to="/register" className="nav-item">Register</Link>
            </li>
            </>
        )}
      </ul>
    </nav>
  );
}

function App() {
  return (
    <Router>
      <div>
        <Navigation />
        <Routes>
          <Route path="/" element={<ItemTable />} />
          <Route path="/edit/:id" element={<EditItem />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
