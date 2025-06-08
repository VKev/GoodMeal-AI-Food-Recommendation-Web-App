import { useEffect, useState } from 'react';
import { S3Client } from '@aws-sdk/client-s3';

export const useAwsSetup = () => {
  const [s3Client, setS3Client] = useState<S3Client | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const setupAwsCredentials = async () => {
      try {
        const sessionCredentials = sessionStorage.getItem('awsCredentials');
        const cachedExpiration = sessionStorage.getItem('awsCredentialsExpiration');

        if (sessionCredentials && cachedExpiration && new Date(cachedExpiration) > new Date()) {
          const credentials = JSON.parse(sessionCredentials);
          console.log("cached",credentials);
          const client = new S3Client({
            region: 'ap-southeast-1', 
            endpoint: 'https://s3.ap-southeast-1.wasabisys.com',
            credentials: {
                accessKeyId: credentials.accessKeyId,
                secretAccessKey: credentials.secretAccessKey,
                sessionToken: credentials.sessionToken,
              },
            forcePathStyle: true,
          });
          setS3Client(client);
          setIsLoading(false);
          return;
        }

        const response = await fetch('/api/resource/get-temporary-credential', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ Name: 'YourSessionName', DurationSeconds: 3600 }), // Customize as needed
        });

        if (!response.ok) {
          throw new Error(`Failed to fetch AWS credentials: ${response.statusText}`);
        }

        const data = await response.json();
        const { accessKeyId, secretAccessKey, sessionToken, expiration } = data.value;

        const credentials = { accessKeyId, secretAccessKey, sessionToken };
        sessionStorage.setItem('awsCredentials', JSON.stringify(credentials));
        sessionStorage.setItem('awsCredentialsExpiration', expiration);

        const client = new S3Client({
          region: 'ap-southeast-1',
          endpoint: 'https://s3.ap-southeast-1.wasabisys.com',
          credentials,
          forcePathStyle: true,
        });

        setS3Client(client);
      } catch (err: any) {
        console.error('Error setting up AWS:', err);
        setError(err.message);
      } finally {
        setIsLoading(false);
      }
    };

    setupAwsCredentials();
  }, []);

  return { s3Client, isLoading, error };
};

export default useAwsSetup;
