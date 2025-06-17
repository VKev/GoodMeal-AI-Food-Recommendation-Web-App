export interface QueryResult<T> {
    isSuccess: boolean;
    value?: T;
    error?: {
        code: string;
        description: string;
    };
} 