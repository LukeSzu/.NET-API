import React, { useState } from 'react';

const useLoginTimeout = () => {
    const [timeoutId, setTimeoutId] = useState(null);

    const setLoginTimeout = (callback, delay) => {
        const newTimeoutId = setTimeout(callback, delay);
        setTimeoutId(newTimeoutId);
    };

    const clearLoginTimeout = () => {
        if (timeoutId) {
            clearTimeout(timeoutId);
            setTimeoutId(null);
        }
    };

    return {
        setLoginTimeout,
        clearLoginTimeout,
    };
};

export default useLoginTimeout;