import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { API_URL } from './config'; // Zaimportuj stałą API_URL

const ItemList = () => {
  const [items, setItems] = useState([]);

  useEffect(() => {
    const fetchItems = async () => {
      try {
        const response = await axios.get(`${API_URL}/items`);  // Wymaga korekty
        setItems(response.data);
      } catch (error) {
        console.error('Error fetching items:', error);
      }
    };

    fetchItems();
  }, []);

  return (
    <div>
      <h2>Lista przedmiotów</h2>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Tytuł</th>
            <th>Opis</th>
            <th>Cena</th>
          </tr>
        </thead>
        <tbody>
          {items.map(item => (
            <tr key={item.id}>
              <td>{item.id}</td>
              <td>{item.title}</td>
              <td>{item.description}</td>
              <td>{item.price}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default ItemList;