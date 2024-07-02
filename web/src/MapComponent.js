import React, { useState, useEffect } from 'react';
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import geocode from './geocode';
import { useParams } from 'react-router-dom';

const MapComponent = () => {
    const {city, street } = useParams();
  const [position, setPosition] = useState(null);

  useEffect(() => {
    const fetchCoordinates = async () => {
      try {
        const coords = await geocode(city, street);
        setPosition([coords.lat, coords.lon]);
      } catch (error) {
        console.error('Error fetching coordinates:', error);
      }
    };

    fetchCoordinates();
  }, [city, street]);

  return (
    <div>
      {position ? (
        <MapContainer center={position} zoom={13} style={{ height: '400px', width: '100%' }}>
          <TileLayer
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          />
          <Marker position={position}>
            <Popup>
              {street}, {city}
            </Popup>
          </Marker>
        </MapContainer>
      ) : (
        <p>Something went wrong with the seller's address. Contact seller using other methods.</p>
      )}
    </div>
  );
};

export default MapComponent;
