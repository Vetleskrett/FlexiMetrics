import type { AnalysisFieldType } from 'src/types/';
import StringFilter from './StringFilter.svelte';
import NumberFilter from './NumberFilter.svelte';
import { createRender } from 'svelte-headless-table';
import { type Writable } from 'svelte/store';
import BoolFilter from './BoolFilter.svelte';
import DateFilter from './DateFilter.svelte';
import type { DateValue } from '@internationalized/date';

const stringFilterFn = ({ filterValue, value }: { filterValue: string; value: any }) =>
    value.toString().toLowerCase().includes(filterValue.toLowerCase());

const boolFilterFn = ({ filterValue, value }: { filterValue: boolean | undefined; value: boolean }) => {
    return filterValue == undefined || value == filterValue;
}

const dateFilterFn = ({
    filterValue,
    value
}: {
    filterValue: { after?: DateValue; before?: DateValue };
    value: Date;
}) => (filterValue.after == undefined || new Date (value) >= filterValue.after.toDate('utc')) && 
    (filterValue.before == undefined || new Date (value) <= filterValue.before.toDate('utc'));

const numberFilterFn = ({
    filterValue,
    value
}: {
    filterValue: { min: number; max: number };
    value: any;
}) => Number(value) >= filterValue.min && Number(value) <= filterValue.max;

const rangeFilterFn = ({
    filterValue,
    value
}: {
    filterValue: { min: number; max: number };
    value: any;
}) => Number(value.Value) >= filterValue.min && Number(value.Value) <= filterValue.max;

export const getFilter = (type: AnalysisFieldType) => {
    switch (type) {
        case 'Integer':
        case 'Float':
            return {
                initialFilterValue: { min: -Infinity, max: Infinity },
                fn: numberFilterFn,
                render: ({ filterValue }: { filterValue: Writable<{ min: number; max: number }> }) =>
                    createRender(NumberFilter, { filterValue })
            };
        case 'Range':
            return {
                initialFilterValue: { min: -Infinity, max: Infinity },
                fn: rangeFilterFn,
                render: ({ filterValue }: { filterValue: Writable<{ min: number; max: number }> }) =>
                    createRender(NumberFilter, { filterValue })
            };
        case 'Boolean':
            return {
                initialFilterValue: undefined,
                fn: boolFilterFn,
                render: ({ filterValue }: { filterValue: Writable<boolean | undefined> }) =>
                    createRender(BoolFilter, { filterValue })
            };
        case 'DateTime':
            return {
                initialFilterValue: {after: undefined, before: undefined},
                fn: dateFilterFn,
                render: ({ filterValue }: { filterValue: Writable<{ after?: DateValue; before?: DateValue }> }) =>
                    createRender(DateFilter, { filterValue })
            };
        case 'List':
            return undefined;
        default:
            return {
                initialFilterValue: '',
                fn: stringFilterFn,
                render: ({ filterValue }: { filterValue: Writable<string> }) =>
                    createRender(StringFilter, { filterValue })
            };

    }
};