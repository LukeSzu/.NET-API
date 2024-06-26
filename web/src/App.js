import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link, useNavigate } from 'react-router-dom';
import ItemTable from './ItemTable';
import EditItem from './EditItem';
import Login from './Login';
import Register from './Register';
import MyItemTable from './MyItems';
import AddItem from './AddItem';
import DetailsItem from './DetailsItem';
import './App.css';
import useLoginTimeout from './loginTimeout';

function Navigation() {
  const username = localStorage.getItem('username');
  const navigate = useNavigate();
  const { clearLoginTimeout } = useLoginTimeout();
  document.title = "Advertising portal"

  const handleLogout = () => {
    localStorage.removeItem('username');
    localStorage.removeItem('token');
    clearLoginTimeout();
    navigate('/login');
  };

  return (
    <nav class ='nav-container'>
      {username ? (
        <span class="hi">Hi, {username}</span>
      ) : (
        <span class="hi">Hi</span>
      )}
      
      <ul>
        <li>
          <Link to="/" className="nav-item">All Items</Link>
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
          <Route path="/myitems" element={<MyItemTable />} />
          <Route path="/additem" element={<AddItem />} />
          <Route path="/details/:id" element={<DetailsItem />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
