import type { AnalysisFieldType } from 'src/types';
import StringFilter from './StringFilter.svelte';
import NumberFilter from './NumberFilter.svelte';
import { createRender } from 'svelte-headless-table';
import { type Writable } from 'svelte/store';

const stringFilterFn = ({ filterValue, value }: { filterValue: string; value: any }) =>
    value.toString().toLowerCase().includes(filterValue.toLowerCase());

const numberFilterFn = ({
    filterValue,
    value
}: {
    filterValue: { min: number; max: number };
    value: any;
}) => Number(value) >= filterValue.min && Number(value) <= filterValue.max;

export const getFilter = (type: AnalysisFieldType) => {
    switch (type) {
        case 'Int':
            return {
                initialFilterValue: { min: -Infinity, max: Infinity },
                fn: numberFilterFn,
                render: ({ filterValue }: { filterValue: Writable<{ min: number; max: number }> }) =>
                    createRender(NumberFilter, { filterValue })
            };
        default:
            return {
                initialFilterValue: '',
                fn: stringFilterFn,
                render: ({ filterValue }: { filterValue: Writable<string> }) =>
                    createRender(StringFilter, { filterValue })
            };
    }
};