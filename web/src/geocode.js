import axios from 'axios';

const geocode = async (city, street) => {
  try {
    const response = await axios.get('https://nominatim.openstreetmap.org/search', {
      params: {
        q: `${street}, ${city}`,
        format: 'json',
        addressdetails: 1,
        limit: 1,
      },
    });
    
    if (response.data && response.data.length > 0) {
      const { lat, lon } = response.data[0];
      return { lat: parseFloat(lat), lon: parseFloat(lon) };
    } else {
      throw new Error('No results found');
    }
  } catch (error) {
    console.error('Error during geocoding:', error);
    throw error;
  }
};

export default geocode;
