export class PageResult<Type> {
    public data!: Type[];
    public total!: number;
}


export enum Direction
{
    Asc = 1,
    Desc = -1
}
export const ORDER_BY_DEFAULT: string = 'UpdatedAt';
export const PAGE_DEFAULT: number = 1;
export const PAGE_SIZE_DEFAULT: number = 10; 